
var hotidolitem;
var num = 0;
var page = 1;
var off_on = true;
var ui;

$(document).ready(function(){
	$(".card-columns").on("click",".card-img-top",function(){
		
	});
	
	ui = window.localStorage.getItem("ui");
	
	hotidolitem = $("#hotidol").children(0).clone();
	$("#hotidol").empty();
	setTimeout(gethotidol(), 30);
	setTimeout(getnewidol(0), 30);
	
    $(window).scroll(function(){
        var windowTop = parseInt($(window).scrollTop());
        var top = parseInt($(document).scrollTop());
        var height = $(window).height();
        var height1 = $(document).height();
        var totalHeight = windowTop + height;

        if((height1 <= (totalHeight + 100) ) && page<= 30){
            if (off_on) {
				off_on = false;			
				setTimeout(getnewidol(page), 30);
	            page++;
	            num++;
	            console.log(page,num);
			}
        }
    });
	
	
	
	
}); 


var hotidolurl = "/Index/Hotidol";

function gethotidol(){
	$.post(hotidolurl, { cpid:null},
	function (data,status) {
		if(data != null){
			listappend($("#hotidol"),data);
			/*
			$(data).each(function(idx,item){
				var hotidol0 = $(hotidolitem).clone();
				$(hotidol0).find(".card-img-top").attr("src",item.picurl);
				$(hotidol0).find(".card_a").attr("href","/Detail/" + item.picurl);
				$("#hotidol").append($(hotidol0));
			})
			*/
		}
	},'json');
}


var newidolurl = "/Index/Newidol";

function getnewidol(pageidx){
	$.post(newidolurl, { pageidx:pageidx},
	function (data,status) {
		if(data != null){
			listappend($("#newidol"),data);
			/*
			$(data).each(function(idx,item){
				var hotidol0 = $(hotidolitem).clone();
				$(hotidol0).find(".card-img-top").attr("src",item.picurl);
				$(hotidol0).find(".card_a").attr("href","/Detail/" + item.picurl);
				$("#newidol").append($(hotidol0));
			});
			*/
			off_on = true;
		}
	},'json');
}


function listappend(container,data){
	$(data).each(function(idx,item){
		var hotidol0 = $(hotidolitem).clone();
		$(hotidol0).find(".card-img-top").attr("src",item.picurl);
		$(hotidol0).find(".card_a").attr("href","/Detail/" + item.picurl + "?guid=" + ui.Online_key);
		$(container).append($(hotidol0));
	});	
}
