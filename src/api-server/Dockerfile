FROM golang:1.19.1-alpine3.16
WORKDIR /go/moretech
COPY . .
RUN go build -o app
EXPOSE 8080
CMD ["/go/moretech/app"]