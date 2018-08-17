package Handler

import (
	"html/template"
	"log"
	"net/http"
)

func Account(w http.ResponseWriter, r *http.Request) {

	t, err := template.ParseFiles(
		"wwwroot/tpl/Register.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Fatal(err)
	}

	data := struct {
		Title string
	}{
		Title: "load common template",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
	//fmt.Fprintf(w, "%s", t.e)
}