import { FormControl, FormGroup } from '@angular/forms';

export class ValidatorForm {
  public static Validate(formGroup: FormGroup): boolean {
    if (!formGroup.valid) {
      Object.keys(formGroup.controls).forEach((field) => {
        const control = formGroup.get(field);
        if (control instanceof FormControl) {
          control.markAsTouched({ onlySelf: true });
        } else if (control instanceof FormGroup) {
          this.Validate(control);
        }
      });
    }
    return formGroup.valid;
  }
}
