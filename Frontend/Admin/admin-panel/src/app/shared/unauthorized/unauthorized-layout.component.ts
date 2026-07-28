import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-unauthorized-layout',
  imports: [RouterOutlet],
  templateUrl: './unauthorized-layout.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UnauthorizedLayoutComponent {}