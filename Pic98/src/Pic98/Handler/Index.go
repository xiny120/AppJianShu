package Handler

import (
	"Pic98/Cfg"
	"bytes"
	"database/sql"
	"encoding/json"
	"html/template"
	"log"
	_ "log"
	"net/http"
	"net/url"
	"strconv"

	_ "github.com/go-sql-driver/mysql"
)

type Pic struct {
	Vcategoryguid string `json:"categoryguid"`
	Vaguid        string `json:"aguid"`
	Vpicurl       string `json:"picurl"`
	Vcreatetime   string `json:"createtime"`
	Vuserguid     string `json:"userguid"`
	Vidolguid     string `json:"idolguid"`
	Vlike         string `json:"like"`
	Vtitle        string `json:"title"`
	Vintro        string `json:"intro"`
	Vtags         string `json:"tags"`
}

func Index(w http.ResponseWriter, r *http.Request) {

	u, err := url.Parse(r.RequestURI)
	if err == nil {

		filePath := "wwwroot/static/www" + u.Path
		log.Println(filePath)
		if pe, _ := FileExists(filePath); pe == true {
			http.ServeFile(w, r, filePath)
			return
		} else {
			t, err := template.ParseFiles(
				"wwwroot/tpl/Index.html",
				"wwwroot/tpl/public/header.html",
				"wwwroot/tpl/public/nav.html",
				"wwwroot/tpl/public/footer.html")

			data := struct {
				Title   string
				Newidol template.HTML
			}{
				Title:   "首页 - 街拍，美腿，丝袜，细高跟，制服,cosplay",
				Newidol: "",
			}

			if err != nil {
				//log.Fatal(err)
			} else {

				db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
				if err != nil {
					//log.Fatal(err)
				} else {
					defer db.Close()
					stmt, _ := db.Prepare(`SELECT aguid,coverimg,likesum,title FROM Pic98.topic order by createtime desc limit ?,?`)
					//log.Println(stmt)
					defer stmt.Close()
					rows, err := stmt.Query(0, 20)
					//log.Println(rows)
					//log.Println(err)
					if err == nil {
						defer rows.Close()
						var buffer bytes.Buffer
						for rows.Next() {
							var pic Pic

							rows.Scan(&pic.Vaguid, &pic.Vpicurl, &pic.Vlike, &pic.Vtitle)
							buffer.WriteString("<div class=\"card p-1 box-cc\"><a href=\"/topic/")
							buffer.WriteString(pic.Vaguid)
							buffer.WriteString(".html\" class=\"card_a\" alt=\"")
							buffer.WriteString(pic.Vtitle)
							buffer.WriteString("\"><img class=\"card-img-top\" src=\"/thumbnail")
							buffer.WriteString(pic.Vpicurl)
							buffer.WriteString("\" alt=\"Card image cap\"></a></div>")

						}
						data.Newidol = template.HTML(buffer.String())
					}
				}
			}

			err = t.Execute(w, data)
			if err != nil {
				//log.Fatal(err)
			}
		}
	}

}

func Index_Hotidol(w http.ResponseWriter, r *http.Request) {
	pagesize := 20
	pageidx := 0
	errf := r.ParseForm()
	if errf != nil {
		//result := "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
	} else {
		pageidx, _ = strconv.Atoi(r.FormValue("pageidx"))
	}

	db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
	if err != nil {
		//log.Fatal(err)
	}
	defer db.Close()

	startidx := pageidx * pagesize

	stmt, _ := db.Prepare(`SELECT categoryguid, aguid, picurl, createtime, idolguid, likesum from picinfo order by likemonth desc limit ?,?`)
	//log.Println(stmt)
	defer stmt.Close()
	rows, err := stmt.Query(startidx, pagesize)
	//log.Println(rows)
	//log.Println(err)
	if err == nil {
		defer rows.Close()

		var pics []Pic
		for rows.Next() {
			var pic Pic

			rows.Scan(&pic.Vcategoryguid, &pic.Vaguid, &pic.Vpicurl, &pic.Vcreatetime, &pic.Vidolguid, &pic.Vlike)
			pics = append(pics, pic)
		}

		usersBytes, _ := json.Marshal(&pics)
		//log.Println(string(usersBytes))
		w.Write(usersBytes)
	}
}

func Index_Newidol(w http.ResponseWriter, r *http.Request) {
	pagesize := 30
	pageidx := 0
	errf := r.ParseForm()
	if errf != nil {
		//result := "{\"status\":1,\"msg\":\"WebApi Account/Register/Cmd ParseForm失败\"}"
	} else {
		pageidx, _ = strconv.Atoi(r.FormValue("pageidx"))
	}

	db, err := sql.Open("mysql", Cfg.Cfg["tidb"])
	if err != nil {
		//log.Fatal(err)
	}
	defer db.Close()
	startidx := pageidx * pagesize
	stmt, _ := db.Prepare(`SELECT aguid,coverimg,createtime,likesum,userguid,idolguid,title,intro,tags FROM Pic98.topic order by createtime desc limit ?,?`)
	//log.Println(stmt)
	defer stmt.Close()
	rows, err := stmt.Query(startidx, pagesize)
	//log.Println(rows)
	//log.Println(err)
	if err == nil {
		defer rows.Close()

		var pics []Pic
		for rows.Next() {
			var pic Pic

			rows.Scan(&pic.Vaguid, &pic.Vpicurl, &pic.Vcreatetime, &pic.Vlike, &pic.Vuserguid, &pic.Vidolguid, &pic.Vtitle, &pic.Vintro, &pic.Vtags)
			pics = append(pics, pic)
		}

		usersBytes, _ := json.Marshal(&pics)
		//log.Println(string(usersBytes))
		w.Write(usersBytes)
	}
}
