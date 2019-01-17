package Handler

import (
	"log"
	"net/http"

	"github.com/deuill/go-php"
)

func Php_ueditor_controller(w http.ResponseWriter, r *http.Request) {

	engine, _ := php.New()

	context, _ := engine.NewContext()
	context.Output = w

	context.Exec("index.php")
	engine.Destroy()

	/*
		var result string
		result = "{\"state\":\"SUCCESS\"}"
		err := r.ParseForm()
		if err != nil {
			result = "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
		} else {
		}
		log.Println(result)
		w.Write([]byte(result))
	*/
}
