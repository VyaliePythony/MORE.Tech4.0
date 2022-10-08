FROM golang:1.19.1-alpine3.16
WORKDIR /go/moretech

WORKDIR /go/moretech/api-server
COPY api-server/ .
WORKDIR /go/moretech/model
COPY model/ .

WORKDIR /go/moretech/model
ENV PYTHONUNBUFFERED=1
RUN apk add --update --no-cache python3 && ln -sf python3 /usr/bin/python
RUN python3 -m ensurepip
RUN pip3 install --no-cache --upgrade pip setuptools
RUN pip3 install -r requirments.txt

WORKDIR /go/moretech/api-server
RUN go build -o app
EXPOSE 8080
CMD ["/go/moretech/api-server/app"]