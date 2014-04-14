//home controller


console.log("Executing the tasks view controller");


var vm = new TasksViewModel();

document.addEventListener("DOMContentLoaded", function() {
  console.log("Binding the tasks view to knockout viewmodel");
  vm.loadTasks();
  ko.applyBindings(vm);
});
