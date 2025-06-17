import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../tasks/task.service';
import { CommentService } from '../comments/comment.service';


@Component({
  selector: 'app-task-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-detail.component.html'
})
export class TaskDetailComponent implements OnInit {
  taskId!: number;
  task: any;
  comments: any[] = [];
  newComment = '';

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private commentService: CommentService
  ) {}

  ngOnInit(): void {
    this.taskId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadTask();
    this.loadComments();
  }

  loadTask(): void {
    this.taskService.getTask(this.taskId).subscribe((task) => {
      this.task = task;
    });
  }

  loadComments(): void {
    this.commentService.getComments(this.taskId).subscribe((comments) => {
      this.comments = comments;
    });
  }

  addComment(): void {
    if (!this.newComment.trim()) return;

    const commentObj = {
      content: this.newComment,
      isUpdated: false
    };

    this.commentService.createComment(this.taskId, commentObj).subscribe({
      next: () => {
        this.newComment = '';
        this.loadComments();
      },
      error: (err) => {
        console.error('❌ Error al crear comentario:', err);
        alert('Error al crear comentario. Verifica que estés autenticado.');
      }
    });
  }
}
