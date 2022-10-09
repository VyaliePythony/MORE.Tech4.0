from dataclasses import dataclass
from typing import List, Tuple

from sklearn.feature_extraction.text import TfidfVectorizer
from natasha import Segmenter, MorphVocab, NewsEmbedding, NewsMorphTagger, Doc
from razdel import tokenize, sentenize
import numpy as np
import pandas as pd
import string
from string import punctuation
from scipy import spatial

import nltk
from nltk.corpus import stopwords
nltk.download('stopwords', quiet=True)

noise = stopwords.words('russian') + list(punctuation) + list('«»')

DATAPATH = "/appdata/"

@dataclass
class Natasha:
    """
    Набор утилит для лемматизации
    """
    segmenter = Segmenter()
    morph_vocab = MorphVocab()
    emb = NewsEmbedding()
    morph_tagger = NewsMorphTagger(emb)

    @staticmethod
    def lemmatization4vectorizer(text: str) -> List[str]:
        doc = Doc(text)
        doc.segment(Natasha.segmenter)
        doc.tag_morph(Natasha.morph_tagger)
        for token in doc.tokens:
            token.lemmatize(Natasha.morph_vocab)
        return [_.lemma for _ in doc.tokens]

    @staticmethod
    def lemmatization4convolution(text: str) -> List[str]:
        doc = Doc(text.translate(str.maketrans('', '', string.punctuation)))
        doc.segment(Natasha.segmenter)
        doc.tag_morph(Natasha.morph_tagger)
        for token in doc.tokens:
            token.lemmatize(Natasha.morph_vocab)
        return [_.lemma for _ in doc.tokens if _.lemma not in noise]

    @staticmethod
    def lemmatization4news_category_predict(text: str):
        return Natasha.lemmatization4convolution(text)

class TextConv:
    def __init__(self, doc_size: int = 5):
        """
        :param doc_size: кол-во предложений в статье после свертки
        """
        self.tfidf_matrix = None
        self.rowmax = np.array([])          # max tfidf in each row
        self.doc_size = doc_size

    def __get_split_sentence(self, sen: str):
        return sentenize(sen)

    def __tokenize(self, sen: str):
        return Natasha.lemmatization4convolution(sen)

    def __vectorize(self, data: pd.DataFrame, title: bool = False):
        if not title:
            self.vectorizer = TfidfVectorizer(tokenizer=Natasha.lemmatization4vectorizer, stop_words=noise)
            self.tfidf_matrix = self.vectorizer.fit_transform(data['text'])
        else:
            self.vectorizer_title = TfidfVectorizer(tokenizer=Natasha.lemmatization4vectorizer, stop_words=noise, ngram_range=(3, 7))
            self.tfidf_matrix_title = self.vectorizer_title.fit_transform(data['title'])

    def __update_res(self, res: List, sen_per_word: List):
        for w in sen_per_word:
            res.append(w)
        return res

    def __compute_tfidf_for_paper(self, doc_index) -> List[Tuple[str, float]]:
        """
        Подсчет tf-idf для одной статьи
        :param doc_index: номер статьи во всем дата сете
        :return: пару слово-tfidf
        """
        feature_names = self.vectorizer.get_feature_names_out()
        feature_index = self.tfidf_matrix[doc_index, :].nonzero()[1][::-1]
        tfidf_score_for_paper = []
        for i in feature_index:
            tfidf_score_for_paper.append((feature_names[i], self.tfidf_matrix[doc_index, i]))
        return tfidf_score_for_paper

    def __find_eq(self, word: str) -> float:
        for i in self.tfidf_score:
            if i[0] == word:
                return i[1]
        return 0.0

    def __compute_tfidf_for_sen(self, sen: List[str]) -> float:
        senlen = len(sen)
        accumulate_tfidf = 0
        for word in sen:
            accumulate_tfidf += self.__find_eq(word)
        return accumulate_tfidf

    def __detail_conv(self, text: str, doc_index: str) -> List[Tuple[int, List[str], float]]:
        """
        Свертка одной статьи. Детали.
        :param text: текст статьи
        :param doc_index: номер в коллекции документов
        :param res_size: кол-во предложений на выходе
        :return: см. сигнатуру функции
        """
        result = [] # (sen_index, sum_tf_idf)
        split_text_per_sen = list(self.__get_split_sentence(text))
        count_sentence = len(split_text_per_sen)
        self.tfidf_score = self.__compute_tfidf_for_paper(doc_index)
        for i in range(count_sentence):
            split_sen_per_word = self.__tokenize(split_text_per_sen[i].text)
            result.append((i, split_sen_per_word, self.__compute_tfidf_for_sen(split_text_per_sen)))
        return result

    def __tiny_conv(self, text: str, doc_index: str, res_size: int):
        """
        Маленькая свертка для одной статьи
        :param text:
        :param doc_index:
        :param res_size:
        :return:
        """
        index_sen_tfidf_vec = self.__detail_conv(text, doc_index)
        top_k = index_sen_tfidf_vec[:res_size]
        result_text = []

        for i in top_k:
            senindex = i[0]
            result_text = self.__update_res(result_text, index_sen_tfidf_vec[senindex][1])
        return result_text

    def __dedup(self, data: pd.DataFrame, remove_nums: int) -> pd.DataFrame:
        """
        Удаление дубликатов новостей
        :param data: коллекция всех статей
        :param remove_nums: сколько убрать статей с хвоста
        :return: обновленную коллекцию без дубликатов
        """
        distance_between = []
        count_paper = data.shape[0]
        for i in range(count_paper):
            for j in range(i + 1, count_paper):
                distance = 1 - spatial.distance.cosine(self.tfidf_matrix[i].toarray()[0], self.tfidf_matrix[j].toarray()[0])
                distance_between.append((i, j, distance))
        distance_between.sort(key=lambda x: -x[2])
        index2delete = []
        for d in range(remove_nums):
            index1 = distance_between[d][0]
            index2 = distance_between[d][1]
            if index1 not in index2delete:
                index2delete.append(index1)
            elif index2 not in index2delete:
                index2delete.append(index2)
        data = data.drop(index = index2delete)
        return data

    def __define_trend(self, data: pd.DataFrame, k: int):
        self.__vectorize(data, title=True)
        feature_name_title = self.vectorizer_title.get_feature_names_out()

        name_idf = []
        size = self.tfidf_matrix_title.shape[0]
        idf_max = self.tfidf_matrix_title.argmax(1)
        for i in range(size):
            name = feature_name_title[idf_max[i].item()]
            tf_value = np.max(self.tfidf_matrix_title[i:,])
            name_idf.append((name, tf_value))
        name_idf.sort(key=lambda x: -x[1])
        result = pd.DataFrame()
        for i in range(k):
            result = result.append({'trend': name_idf[i][0]}, ignore_index=True)
        result.to_csv(DATAPATH+'actual_trend.csv', index = False)

        result_for_inside = pd.DataFrame()
        for i in range(len(name_idf)):
            result_for_inside = result_for_inside.append({'trend': name_idf[i][0],
                                                          'tfidf': str(name_idf[i][1])},
                                                         ignore_index=True)
        result_for_inside.to_csv(DATAPATH+'inside_trend.csv', index = False)

    def compute(self, data: pd.DataFrame):
        """
        Старт предобработки
        :param data:
        :return:
        """
        self.__define_trend(data, 50)
        self.__vectorize(data, title=False)
        text = data['text'].to_numpy()

        for i, t in enumerate(text):
            data['text'][i] = ' '.join(self.__tiny_conv(t, i, self.doc_size))
        data = self.__dedup(data, 10)
        return data
