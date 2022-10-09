from typing import List
from text import Natasha
import pandas as pd

available_hipotize = [
    'Увеличение импорта товара в индию повысит маржинальность бизнеса'
]

class InsideUtils:
    @staticmethod
    def lemmatize(text):
        return Natasha.lemmatization4news_category_predict(text)

    @staticmethod
    def find_near(hip: str, inside_trend: pd.DataFrame):
        #
        #   formula: count_eq_word * tfidf_this_trend / tfidf_all_trend * 100
        #
        count_eq_word = 0 # кол-во одинаковых слов в тренде и гипотезе
        tfidf_all_trend = inside_trend['tfidf'].sum()
        index_count_x_tfidf = []
        index = 0
        for trend, tfidf in inside_trend.values:
            split_trend = trend.split()
            for h in hip:
                if h in split_trend:
                    count_eq_word += 1
            index_count_x_tfidf.append((index, count_eq_word * tfidf / (tfidf_all_trend * 100)))
            count_eq_word = 0
            index += 1
        index_count_x_tfidf.sort(key = lambda x: -x[1])
        top_index = []
        for i in range(3):
            top_index.append(index_count_x_tfidf[i][0])
        return inside_trend.loc[inside_trend.index[top_index]]



class Inside:
    def choice_hipotize(self):
        print("Enter 0 to choose random hipotize.", "Enter 1 to input hipotize", sep = '\n')
        return str(input())

    def enter_get_hipotize(self):
        hip_text = input("Enter hipotize below\n")
        return InsideUtils.lemmatize(hip_text)

    def get_inside(self):
        answer = self.choice_hipotize()
        if str(answer) == '1':
            lemma_hip = self.enter_get_hipotize()
        else:
            lemma_hip = InsideUtils.lemmatize(available_hipotize[0])
            print("H0: ", available_hipotize[0], end = '\n\n\n')

        # read inside_trend
        inside_trend = pd.read_csv('inside_trend.csv', index_col=False)
        res = InsideUtils.find_near(lemma_hip, inside_trend)
        print("Я котик! У меня лапки и это все что я смог проанализировать :)", end = '\n\n')
        for (trend, value) in res.values:
            print(trend, " Вероятность успеха гипотезы = ", value)