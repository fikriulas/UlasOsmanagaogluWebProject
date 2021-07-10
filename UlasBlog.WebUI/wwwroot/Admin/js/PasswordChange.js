
const OldToggle = document.querySelector('#OldToggle');
const OldPassword = document.querySelector('#OldPassword');
OldToggle.addEventListener('click', function (e) {
    // toggle the type attribute
    const typea = OldPassword.getAttribute('type') === 'password' ? 'text' : 'password';
    OldPassword.setAttribute('type', typea);
    // toggle the eye / eye slash icon
    if (document.getElementById("OldToggle").className === 'fas fa-eye-slash') {
        document.getElementById("OldToggle").className = 'fa fa-fw fa-eye';
    }
    else {
        document.getElementById("OldToggle").className = 'fas fa-eye-slash';
    }
});


const newToggle = document.querySelector('#newToggle');
const NewPassword = document.querySelector('#NewPassword');
newToggle.addEventListener('click', function (e) {
    // toggle the type attribute
    const typeb = NewPassword.getAttribute('type') === 'password' ? 'text' : 'password';
    NewPassword.setAttribute('type', typeb);
    // toggle the eye / eye slash icon
    if (document.getElementById("newToggle").className === 'fas fa-eye-slash') {
        document.getElementById("newToggle").className = 'fa fa-fw fa-eye';
    }
    else {
        document.getElementById("newToggle").className = 'fas fa-eye-slash';
    }
});


const confirmToggle = document.querySelector('#confirmToggle');
const ConfirmPassword = document.querySelector('#ConfirmPassword');
confirmToggle.addEventListener('click', function (e) {
    // toggle the type attribute
    const typec = ConfirmPassword.getAttribute('type') === 'password' ? 'text' : 'password';
    ConfirmPassword.setAttribute('type', typec);
    // toggle the eye / eye slash icon
    if (document.getElementById("confirmToggle").className === 'fas fa-eye-slash') {
        document.getElementById("confirmToggle").className = 'fa fa-fw fa-eye';
    }
    else {
        document.getElementById("confirmToggle").className = 'fas fa-eye-slash';
    }
});
