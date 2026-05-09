import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'maxLengthMessage',
})
export class MaxLengthMessage implements PipeTransform {

  constructor(){}

  transform(value: any, fieldName: string, maxLength: number) {
    return `${fieldName} cannot exceed ${maxLength} characters`;
  }
}
