import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Home } from './components/home/home';
import { Login } from './components/login/login';
import {AuthGuard} from './helpers/auth.guard';

const routes: Routes = [
  { path: '', component: Home, canActivate: [AuthGuard] },
  { path: 'login', component: Login },

  // otherwise redirect to home
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
