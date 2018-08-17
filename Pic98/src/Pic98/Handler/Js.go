package Handler

import (
	"log"
	"net/http"
	"strings"
)

func Js(w http.ResponseWriter, r *http.Request) {
	param := strings.Split(r.RequestURI, "/")
	if len(param) >= 3 {
		log.Println(len(param))
		log.Println(param[2])
		http.ServeFile(w, r, "wwwroot/js/"+param[2])
	}
}