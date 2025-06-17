import { Component, Input, OnInit } from '@angular/core';
import { CommentService } from './comment.service';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [],
  templateUrl: './comment-list.component.html'
})
export class CommentListComponent implements OnInit {
  @Input() taskId!: number;
  comments: any[] = [];

  constructor(private commentService: CommentService) {}

  ngOnInit(): void {
    this.commentService.getComments(this.taskId).subscribe(data => {
  this.comments = data;
});
  }
}
