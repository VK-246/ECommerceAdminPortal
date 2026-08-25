import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AiChatComponent } from './ai-chat.component';

const routes: Routes = [{ path: '', component: AiChatComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AiChatRoutingModule { }
