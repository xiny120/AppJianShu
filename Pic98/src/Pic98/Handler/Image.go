package Handler

import (
	"log"
	"net/http"
	"strings"
)

func Image(w http.ResponseWriter, r *http.Request) {
	//fmt.Fprintf(w, "%s", "Image now!")
	log.Println(r.RequestURI)
	param := strings.Split(r.RequestURI, "/")
	log.Println(param)
	http.ServeFile(w, r, "wwwroot/Image/timg.jpg")
}
