import sys
from model import Model
import preprocessing
import os.path

PARSED_DATA = "data.csv"
DATASET = "dataset.csv"
DATAPATH = "/go/moretech/appdata/"

postModel = Model()

def main(query_type, role):
    if not os.path.isfile(DATAPATH+DATASET):
        preprocessing.preprocess()
    if query_type == "digest":
        res = postModel.news(role, 3)
    elif query_type == "trends":
        res = postModel.trends()
    elif query_type == "preprocess":
        preprocessing.preprocess()
        res = "data preprocessed"
    else:
        print("main.py ERROR : unknown query")
        return 1
    print(res)
    return 0

if __name__ == "__main__":
    main(sys.argv[1],sys.argv[2])