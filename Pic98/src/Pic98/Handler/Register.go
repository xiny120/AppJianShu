package Handler

import (
	"html/template"
	"log"
	"net/http"
)

func Register(w http.ResponseWriter, r *http.Request) {
	//fmt.Fprintf(w, "%s", "register now!")
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
		Title: "注册新用户",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
}
