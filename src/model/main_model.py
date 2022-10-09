from model import Model
import pandas as pd


if __name__ == '__main__':
    model = Model()
    res = model.news('manager', 3)
    print(*res[0]['title'])
    print(*res[1]['title'])
    print(*res[2]['title'])
    # model = Model()
    # model.update('data.csv')
    # model = Model()
    # model.add_role('manager')


