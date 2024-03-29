package scripts

import (
	"bufio"
	"io"
	"log"
	"os"
	"os/exec"
	"path"
	"path/filepath"
)

var result string

func Execute(Trends string, Role string) string {
	directory, err := os.Getwd()
	if err != nil {
		log.Println(err)
	}
	cmd := exec.Command("python3", path.Join(filepath.Dir(directory), "/model", "main.py"), Trends, Role)
	stdout, err := cmd.StdoutPipe()
	if err != nil {
		panic(err)
	}
	err = cmd.Start()
	if err != nil {
		panic(err)
	}
	go copyOutput(stdout)
	cmd.Wait()
	return result

}
func copyOutput(r io.Reader) {
	scanner := bufio.NewScanner(r)
	for scanner.Scan() {
		result = scanner.Text()
	}
}
