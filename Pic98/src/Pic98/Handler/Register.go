package Handler

import (
	"fmt"

	"net/http"
)

func Register(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, "%s", "register now!")
}
