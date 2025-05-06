
function confirmDelete(i){
    if (confirm('Tem certeza que deseja deletar?')){
        deleteTodo(i);
    }
}
function deleteTodo(i){
    $.ajax({
        url: 'Home/Delete',
        type: 'POST',
        data: {
            id: i
        },
        success: function(){
            window.location.reload();
        }
        
    });
}


