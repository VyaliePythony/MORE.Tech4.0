FROM golang:1.19.2-bullseye
WORKDIR /go/moretech

WORKDIR /go/moretech/api-server
COPY api-server/ .
WORKDIR /go/moretech/model
COPY model/ .

RUN apt-get update
RUN apt-get -y install python3
RUN apt-get -y install python3-setuptools
RUN apt-get -y install python3-pip

WORKDIR /go/moretech/model
RUN pip3 install -r requirments.txt

WORKDIR /go/moretech/api-server
RUN go build -o app
EXPOSE 8080
CMD ["/go/moretech/api-server/app"]