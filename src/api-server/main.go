package api

import (
	"io/ioutil"
	"net/http"

	"github.com/gin-gonic/gin"
)

type responseDescription struct {
	Role int `json:"Role"	`
}

func addTodo(context *gin.Context) {
	body := context.Request.Body
	x, _ := ioutil.ReadAll(body)
	// jsonData, err := json.Marshal(response)
	// if err != nil {
	// 	log.Fatal(err)
	// }
	result := Execute(string(x))
	context.IndentedJSON(http.StatusCreated, result)
}

func main() {
	router := gin.Default() //our server
	// router.GET("/search", getTodos)

	router.POST("/search", addTodo)

	router.Run(":8080")

}
