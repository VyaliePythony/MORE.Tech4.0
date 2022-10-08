# model.py shell, executable by golang server. Sort posts or trends
import sys
import model

def main(query_type, role):
    if query_type == "digest":
        res = model.digest(role)
        print(res)
        return 0
    elif query_type == "trends":
        res = model.trends()
        print(res)
        return 0
    else:
        print("Error, unknow query")
        return 1

if __name__ == "__main__":
    main(sys.argv[1],sys.argv[2])