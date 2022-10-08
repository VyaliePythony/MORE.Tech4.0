import sys
import model
import preprocessing
import os.path

PARSED_DATA = "data.csv"
DATASET = "dataset.csv"
DATAPATH = "/go/moretech/appdata/"

def main(query_type=None, role=None):
    if not os.path.isfile(DATAPATH+DATASET):
        preprocessing.preprocess()
    if query_type == "digest":
        res = model.digest(role)
        print(res)
        return 0
    elif query_type == "trends":
        res = model.trends()
        print(res)
        return 0
    elif query_type == "preprocess":
        print(preprocessing.preprocess())
    else:
        print("Error, unknow query")
        return 1

if __name__ == "__main__":
    main(sys.argv[1],sys.argv[2])