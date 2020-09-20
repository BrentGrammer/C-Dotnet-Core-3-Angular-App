import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder,
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { User } from 'src/app/_models/user';
import { Router } from '@angular/router';

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
  bsConfig: Partial<BsDatepickerConfig>; // we only want to set the theme color and in this type other props are set as not optional - so use partial to only require that to be set
  user: User;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.bsConfig = {
      containerClass: 'theme-red', // for color of ngx-bootstrap datepicker
    };
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
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value); // not sure why this is done? immutability?
      this.authService.register(this.registerForm.value).subscribe(
        () => {
          this.alertify.success('Registered');
        },
        (error) => {
          this.alertify.error(error);
        },
        () => {
          // this is the complete callback offered as a third arg in the subscribe flow: log user in after register and redirect
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
