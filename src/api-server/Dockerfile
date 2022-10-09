FROM golang:1.19.2-bullseye AS base
RUN apt-get update
RUN apt-get -y install python3
RUN apt-get -y install python3-setuptools
RUN apt-get -y install python3-pip

WORKDIR /go/moretech/model
COPY model/requirments.txt .
RUN pip3 install -r requirments.txt


FROM base AS build
WORKDIR /go/moretech/api-server
COPY api-server/ .
WORKDIR /go/moretech/model
COPY model/ .


FROM build AS final
WORKDIR /go/moretech/api-server
RUN go build -o app
EXPOSE 8080
CMD ["/go/moretech/api-server/app"]