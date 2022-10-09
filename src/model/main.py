import sys
from model import Model
import os.path
import json
import pandas as pd

PARSED_DATA = "data.csv"
DATASET = "for_rec.csv"
DATAPATH = "/appdata/"

postModel = Model()

def main(query_type, role):
    if not os.path.isfile(DATAPATH+DATASET):
        postModel.update(PARSED_DATA)
    if query_type == "digest":
        df = postModel.news(role, 3)
        res = json.dumps(df.to_dict('records'), ensure_ascii=False)
    elif query_type == "trends":
        trends = postModel.trends()
        df = pd.DataFrame(data=trends.to_list(), columns=['trends'])
        res = json.dumps(df.to_dict('records'), ensure_ascii=False)
    elif query_type == "preprocess":
        postModel.update(PARSED_DATA)
        res = "data preprocessed"
    else:
        print("main.py ERROR : unknown query")
        return 1
    print(res)
    return 0

if __name__ == "__main__":
    main(sys.argv[1],sys.argv[2])