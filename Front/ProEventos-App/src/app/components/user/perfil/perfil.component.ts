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
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss'],
})
export class PerfilComponent implements OnInit {
  form!: FormGroup;
  showSpinner: boolean = false;

  get f(): any {
    return this.form.controls;
  }

  constructor(private fb: FormBuilder, private spinner: NgxSpinnerService) {}

  ngOnInit() {
    this.validation();
  }

  public validation() {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('senha', 'confirmarSenha'),
    };

    this.form = this.fb.group(
      {
        titulo: ['', Validators.required],
        primeiroNome: ['', Validators.required],
        ultimoNome: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        telefone: ['', Validators.required],
        funcao: ['', Validators.required],
        descricao: ['', Validators.required],
        senha: ['', Validators.minLength(6)],
        confirmarSenha: ['', Validators.nullValidator],
      },
      formOptions
    );
  }

  public resetForm(event: any): void {
    event.preventDefault();
    this.form.reset();
  }

  public onSubmit() {
    if (ValidatorForm.Validate(this.form)) {
      this.spinner.show();
    }
  }
}
