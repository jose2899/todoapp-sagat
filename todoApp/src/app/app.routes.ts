import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login.component';
import { TaskListComponent } from './tasks/task-list.component';
import { TaskDetailComponent } from './tasks/task-detail.component';
import { AuthGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'tasks', component: TaskListComponent, canActivate: [AuthGuard] },
  { path: 'tasks/:id', component: TaskDetailComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'login' }
];
