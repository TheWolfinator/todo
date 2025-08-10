import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TagDto } from '../web-api-client';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html'
})
export class FilterComponent {
  @Input() tagSuggestions: TagDto[] = [];

  selectedTags: string[] = [];
  title: string = '';

  @Output() filterChanged = new EventEmitter<{ tags: string[], title: string }>();

  applyFilter() {
    this.filterChanged.emit({
      tags: this.selectedTags,
      title: this.title
    });
  }

  clearFilter() {
    this.selectedTags = [];
    this.title = '';
    this.filterChanged.emit({
      tags: [],
      title: ''
    });
  }
}
