import { FormArray, FormControl, FormGroup } from '@angular/forms';

export class ValidatorForm {
  public static Validate(formGroup: any): boolean {
    if (!formGroup.valid) {
      Object.keys(formGroup.controls).forEach((field) => {
        const control = formGroup.get(field);
        if (control instanceof FormControl) {
          control.markAsTouched({ onlySelf: true });
        } else if (control instanceof FormGroup) {
          this.Validate(control);
        } else if (control instanceof FormArray) {
          this.Validate(control);
      }
      });
    }
    return formGroup.valid;
  }
}
