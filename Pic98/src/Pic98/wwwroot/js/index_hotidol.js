
var hotidolitem;
var num = 0;
var page = 1;
var off_on = true

$(document).ready(function(){
	hotidolitem = $("#hotidol").children(0).clone();
	$("#hotidol").empty();
	setTimeout(gethotidol(), 30);
	setTimeout(getnewidol(0), 30);
	
	/*
	
	$(window).scroll(function () {
        var s = $(document).scrollTop(),
            h = $(window).height(),
            documentH = $(document).height();
        if ( s > (documentH - h) && page<= 30) {
            if (off_on) {
				off_on = false;
				getnewidol(page);
				//setTimeout(getnewidol(page), 30);
		        page++;
		        num++;					
            }
        }
	});
	*/
	
	
	
    $(window).scroll(function(){
        var windowTop = parseInt($(window).scrollTop());
        var top = parseInt($(document).scrollTop());
        var height = $(window).height();
        var height1 = $(document).height();

        console.log('window距离顶部距离:',windowTop);
        console.log('文档距离顶部距离:',top);
        console.log('window高度:',height);
        console.log('文档的高度:',height1);

        var totalHeight = windowTop + height;

        if((height1 <= (totalHeight + 100) ) && page<= 30){
            if (off_on) {
				off_on = false;			
				setTimeout(getnewidol(page), 30);
	            page++;
	            num++;
	            console.log(page,num);
			}
            //$('#main').append(`<div style='background-color : red;height: 100px;margin-top: 10px'>${num}</div>`);
			
        }
    });
	
	
	
	
}); 


var hotidolurl = "/Index/Hotidol";

function gethotidol(){
	$.post(hotidolurl, { cpid:null},
	function (data,status) {
		if(data != null){
			$(data).each(function(idx,item){
				var hotidol0 = $(hotidolitem).clone();
				$(hotidol0).find(".card-img-top").attr("src",item.picurl);
				$("#hotidol").append($(hotidol0));
			})
		}
	},'json');
}


var newidolurl = "/Index/Newidol";

function getnewidol(pageidx){
	$.post(newidolurl, { pageidx:pageidx},
	function (data,status) {
		if(data != null){
			$(data).each(function(idx,item){
				var hotidol0 = $(hotidolitem).clone();
				$(hotidol0).find(".card-img-top").attr("src",item.picurl);
				$("#newidol").append($(hotidol0));
			});
			off_on = true;
		}
	},'json');
}

