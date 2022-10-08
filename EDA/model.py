from typing import List, Tuple

class Model:
    def __init__(self):
        pass

    def update(self) -> bool:
        """
        Запускает скрипт предобработки текста. Обновляет выборку для дайджеста.
        :return: true or false в зависимости от результата
        """
        pass

    def news(self, role: str, n_news: int) -> List[Tuple[str, str, str]]:
        """
        Получить метаинформацию о новостях и их содержание
        :param role: доступная роль. Для тестирования хакатона 'сhief' || 'accountant'
        :param n_news: кол-во новостей в ответе
        :return: см. на сигнатуру функции
        """
        pass

    def trends(self) -> List[str]:
        """
        :return: 5 популярных трендов
        """
        pass

    def add_role(self):
        """
        Добавить роль для предсказания.
        :return:
        """
        pass
