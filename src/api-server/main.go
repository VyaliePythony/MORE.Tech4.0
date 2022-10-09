package main

import (
	"net/http"

	"example.com/api-server/scripts"
	"github.com/gin-gonic/gin"
)

type responseDescription struct {
	Role   string `json:"role"	`
	RequestType string `json:"requestType"	`
}

func addTodo(context *gin.Context) {
	var Thing responseDescription
	if err := context.BindJSON(&Thing); err != nil {
		return
	}
	result := scripts.Execute(Thing.RequestType, Thing.Role)

	// body := context.Request.Body
	// x, _ := ioutil.ReadAll(body)

	// result := Execute(string(x))
	context.String(http.StatusCreated, result)
}

func main() {
	router := gin.Default() //our server
	// router.GET("/search", getTodos)

	router.POST("/search", addTodo)

	router.Run(":8080")

}
