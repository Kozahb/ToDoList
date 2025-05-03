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

function populateForm(i){
    $.ajax ({
        url: 'Home/populateForm',
        type: 'GET',
        data: {
            id: i
        },
        dataType: 'json',
        success: function (response){
            $("#Todo_Name").val(response.name);
            $("#Todo_Id").val(response.id);
            $("#form-button").val("Update Todo");
            $("#form-action").attr("action", "/Home/Update");
        }
    });
}

function submitTodo() {
    const id = $('#Todo_Id').val();
    const name = $('#Todo_Name').val();

    const data = { Id: id, Name: name };

    const url = id ? '/Home/Update' : '/Home/Insert'; 

    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function() {
            console.log("Task updated successfully");
            location.reload(); 
        },
        error: function(xhr, status, error) {
            console.error("Error in submitTodo:", error); 
        }
    });
}