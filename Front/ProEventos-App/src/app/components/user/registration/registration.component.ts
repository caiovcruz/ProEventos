import { Component, OnInit } from '@angular/core';
import {
  AbstractControlOptions,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { ValidatorForm } from '@app/helpers/ValidatorForm';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'],
})
export class RegistrationComponent implements OnInit {
  form!: FormGroup;

  get f(): any {
    return this.form.controls;
  }

  constructor(private fb: FormBuilder, private spinner: NgxSpinnerService) {}

  ngOnInit(): void {
    this.validation();
  }

  public validation(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('senha', 'confirmarSenha'),
    };

    this.form = this.fb.group(
      {
        primeiroNome: ['', Validators.required],
        ultimoNome: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        usuario: ['', [Validators.required, Validators.minLength(3)]],
        senha: ['', [Validators.required, Validators.minLength(6)]],
        confirmarSenha: ['', Validators.required],
        termosUso: ['', Validators.requiredTrue],
      },
      formOptions
    );
  }

  public onSubmit() {
    if (ValidatorForm.Validate(this.form)) {
      this.spinner.show();
    }
  }
}
