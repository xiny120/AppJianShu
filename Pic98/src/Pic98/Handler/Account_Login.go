package Handler

import (
	"html/template"
	"log"
	"net/http"
)

func Login(w http.ResponseWriter, r *http.Request) {
	//fmt.Fprintf(w, "%s", "register now!")
	t, err := template.ParseFiles(
		"wwwroot/tpl/Login.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Fatal(err)
	}

	data := struct {
		Title string
	}{
		Title: "用户登陆",
	}

	err = t.Execute(w, data)
	if err != nil {
		log.Fatal(err)
	}
}
