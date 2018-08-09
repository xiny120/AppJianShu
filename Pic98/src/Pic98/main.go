// main.go
package main

import (
	"log"
	"net/http"
)
import (
	"Pic98/Handler"
)

func main() {

	sMux := http.NewServeMux()
	rh := http.RedirectHandler("http://www.baidu.com", 307)
	sMux.Handle("/baidu", rh)
	regh := http.HandlerFunc(Handler.Register)
	sMux.Handle("/Account/Register/", regh)
	rega := http.HandlerFunc(Handler.Account)
	sMux.Handle("/Account/", rega)
	regi := http.HandlerFunc(Handler.Image)
	sMux.Handle("/Image/", regi)

	log.Println("Listening...")
	http.ListenAndServe(":3000", sMux)

}
