package Handler

import (
	"Pic98/Cfg"
	"database/sql"
	"html/template"
	"log"
	"net/http"
	"regexp"

	_ "github.com/Unknwon/goconfig"
)

func Topic(w http.ResponseWriter, r *http.Request) {

	t, err := template.ParseFiles(
		"wwwroot/tpl/Topic.html",
		"wwwroot/tpl/public/header.html",
		"wwwroot/tpl/public/nav.html",
		"wwwroot/tpl/public/footer.html")
	if err != nil {
		log.Println(err)
	}

	data := struct {
		Title    string
		Listtype string
		Topic    template.HTML
		Any      string
	}{
		Title:    "列表",
		Listtype: "",
		Topic:    "test",
		Any:      "",
	}

	flysnowRegexp := regexp.MustCompile(`^/topic/([\w-]+).html$`)
	params := flysnowRegexp.FindStringSubmatch(r.RequestURI)

	if len(params) > 1 {
		topicguid := params[1]
		log.Println(topicguid)
		cookie, err := r.Cookie("token")
		Any := "未认证用户！游客模式！"
		if err == nil {
			cookievalue := cookie.Value
			//log.Println(cookievalue)
			Any = cookievalue
		}

		data.Any = Any
		db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
		if err != nil {
			log.Println(err)
		} else {
			defer db.Close()
			stmt, _ := db.Prepare(`SELECT createtime,likesum,userguid,idolguid,title,content,tags FROM Pic98.topic where aguid=?`)
			defer stmt.Close()
			rows, err := stmt.Query(topicguid)
			if err == nil {
				defer rows.Close()
				var createtime string
				var like string
				var userguid string
				var idolguid string
				var title string
				var content string
				var tags string
				for rows.Next() {
					rows.Scan(&createtime, &like, &userguid, &idolguid, &title, &content, &tags)
				}
				data.Topic = template.HTML(content)
			}
		}

		err = t.Execute(w, data)
		if err != nil {
			log.Println(err)
		}

	}

	//fmt.Fprintf(w, "%s", t.e)
}
