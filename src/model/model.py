from dataclasses import dataclass
from typing import List, Tuple

import numpy as np

from text import TextConv, Natasha
import pandas as pd

import warnings
warnings.filterwarnings("ignore")

PARSED_DATA = "data.csv"
DATASET = "for_rec.csv"
DATAPATH = "/appdata/"

class ModelUtil:
    @staticmethod
    def find_relevant(data: pd.DataFrame, role_descr: np.ndarray, k: int):
        size = data.shape[0]
        role_size = len(role_descr)
        top = []
        for i in range(size):
            count_true = 0  # кол-во совпадений
            paper_split = data['text'][i].split()
            for j in range(role_size):
                if role_descr[j] in paper_split:
                    count_true += 1
            top.append((count_true, i))
        top.sort(key = lambda x: -x[0])
        top_k = []
        for i in range(k):
            top_k.append(top[i][1])

        return data.loc[data.index[top_k]][['link','title','date']]


class Model:
    def __init__(self, doc_size: int = 5):
        self.text_conv = TextConv(doc_size=doc_size)
        role_list = pd.read_csv(DATAPATH+'available_role.csv')
        self.available_role = role_list['role_name'].to_numpy()

    def update(self, filename: str) -> bool:
        """
        Запускает скрипт предобработки текста. Обновляет выборку для дайджеста.
        :return: true or false в зависимости от результата
        """
        # update recomm base
        labels = ['link', 'title', 'text', 'date']
        df = pd.read_csv(DATAPATH+filename, sep = ';', names = labels, index_col=False)
        df = self.text_conv.compute(df)
        df.to_csv(DATAPATH+DATASET, index = False)

        for role in self.available_role:
            role_tag = pd.read_csv(DATAPATH + role + '.csv', index_col =False)['token']
            buffer = []
            unique_value = []
            for token in role_tag.values:
                lemma = Natasha.lemmatization4news_category_predict(token)
                if len(lemma) > 0:
                    if lemma[0] not in buffer:
                        unique_value.append(lemma[0])
                        buffer.append(lemma[0])
            unique_value = np.array(unique_value).reshape((-1, 1))

            new_role_tag = pd.DataFrame(np.arange(0, len(unique_value)).reshape(-1, 1))
            new_role_tag['token'] = unique_value
            new_role_tag.to_csv(DATAPATH + role + '.csv', index = False)


    def news(self, role: str, n_news: int) -> List[Tuple[str, str, str]]:
        """
        Получить метаинформацию о новостях и их содержание
        :param role: доступная роль. Для тестирования хакатона 'сhief' || 'accountant'
        :param n_news: кол-во новостей в ответе
        :return: см. на сигнатуру функции
        """
        df = pd.read_csv(DATAPATH+DATASET, index_col=False)
        role_descr = pd.read_csv(DATAPATH + role + '.csv', index_col=False)['token'].to_numpy()
        result = ModelUtil.find_relevant(df, role_descr, n_news)

        # TODO: написать сортировку
        return result

    def trends(self) -> List[str]:
        """
        :return: 5 популярных трендов
        """
        trends_ = pd.read_csv(DATAPATH+'actual_trend.csv')['trend']
        index = np.random.choice(range(trends_.shape[0]), 10, replace=False)
        return trends_.loc[trends_.index[index]]

    def add_role(self, new_role):
        """
        Добавить роль для предсказания.
        :return:
        """
        if new_role in self.available_role:
            print("Role existed")
            return

        df = pd.DataFrame()
        print("Enter role features...", "Enter 0 to finish", sep = '\n')
        feature = input()
        while(feature != str(0)):
            df = df.append({'token': feature}, ignore_index=True)
            feature = input()
        curr_role = pd.read_csv(DATAPATH+'available_role.csv', index_col=False)
        curr_role = curr_role.append({'role_name': new_role}, ignore_index=True)
        df.to_csv(DATAPATH + new_role + '.csv', index = False)
        curr_role.to_csv(DATAPATH+'available_role.csv', index = False)

        role_list = pd.read_csv(DATAPATH+'available_role.csv', index_col=False)
        self.available_role = role_list['role_name'].to_numpy()