package Handler

import (
	"fmt"

	"net/http"
)

func Account(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, "%s", "<html>Account now!<img src='/Image/test.png' /></html>")
}
