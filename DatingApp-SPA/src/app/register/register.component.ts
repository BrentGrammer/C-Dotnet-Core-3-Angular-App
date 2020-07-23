import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  // reactive form set up
  registerForm: FormGroup;
  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'], // radio button - set default choice
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(4),
            Validators.maxLength(8),
          ],
        ],
        confirmPassword: ['', Validators.required],
      },
      { validator: this.passwordMatchValidator }
    );
  }

  //custom validator: pass in the formgroup for access to compare fields
  passwordMatchValidator(g: FormGroup) {
    const match =
      g.get('password').value === g.get('confirmPassword').value
        ? null
        : { mismatch: true };

    return match;
  }

  register() {
    // this.authService.register(this.model).subscribe(
    //   () => {
    //     this.alertify.success('registered');
    //   },
    //   (error) => {
    //     this.alertify.error(error);
    //   }
    // );
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
