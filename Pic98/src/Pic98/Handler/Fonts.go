package Handler

import (
	"log"
	"net/http"
	"strings"
)

func Fonts(w http.ResponseWriter, r *http.Request) {
	log.Println(r.RequestURI)
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 3 {
		filePath := "wwwroot/fonts/" + param[2]
		log.Println(filePath)
		if pe, _ := PathExists(filePath); pe == true {
			http.ServeFile(w, r, filePath)
		}
	}
}
