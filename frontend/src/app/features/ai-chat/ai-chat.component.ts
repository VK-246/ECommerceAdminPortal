import { Component } from '@angular/core';
import { AiService } from '../../core/services/ai.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-ai-chat',
  standalone: false,
  templateUrl: './ai-chat.component.html',
  styleUrls: ['./ai-chat.component.scss']
})
export class AiChatComponent {
  prompt = '';
  response = '';
  isLoading = false;

  constructor(private aiService: AiService, private snackBar: MatSnackBar) {}

  askAi(): void {
    if (!this.prompt.trim()) return;

    this.isLoading = true;
    this.response = '';

    this.aiService.getMarketingAdvice(this.prompt).subscribe({
      next: (res) => {
        this.response = res.data;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.message || 'Failed to get AI response';
        this.snackBar.open(msg, 'Close', { duration: 3000 });
      }
    });
  }
}
