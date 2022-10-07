package main

import (
	"net/http"

	"github.com/gin-gonic/gin"
)

type responseDescription struct {
	Role   string `json:"role"	`
	Trends string `json:"newsOrTrand"	`
}

func addTodo(context *gin.Context) {
	var Thing responseDescription
	if err := context.BindJSON(&Thing); err != nil {
		return
	}
	result := Execute(Thing.Trends, Thing.Role)

	// body := context.Request.Body
	// x, _ := ioutil.ReadAll(body)

	// result := Execute(string(x))
	context.IndentedJSON(http.StatusCreated, result)
}

func main() {
	router := gin.Default() //our server
	// router.GET("/search", getTodos)

	router.POST("/search", addTodo)

	router.Run(":8080")

}
