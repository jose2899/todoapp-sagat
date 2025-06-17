import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { TaskService } from './task.service';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './task-list.component.html'
})
export class TaskListComponent implements OnInit {
  tasks: any[] = [];
  newTaskTitle = '';
  newTaskDescription = '';

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe(data => (this.tasks = data));
  }

  createTask(): void {
    this.taskService.createTask({
      title: this.newTaskTitle,
      description: this.newTaskDescription,
      isCompleted: false
    }).subscribe(() => {
      this.newTaskTitle = '';
      this.newTaskDescription = '';
      this.loadTasks();
    });
  }

  deleteTask(id: number): void {
    this.taskService.deleteTask(id).subscribe(() => this.loadTasks());
  }

  toggleCompleted(task: any): void {
    this.taskService.updateTask(task.id, { ...task }).subscribe();
  }
}