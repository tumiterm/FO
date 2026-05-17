
document.addEventListener('DOMContentLoaded', function () {
    const lookupInput = document.getElementById('lookupIdentity');
    const lookupState = document.getElementById('lookupState');
    const lookupHelp = document.getElementById('lookupHelp');
    const lookupFormatBadge = document.getElementById('lookupFormatBadge');

    const idInput = document.getElementById('IDNumber');
    const passportInput = document.getElementById('PassportNumber');
    const dobInput = document.getElementById('DateOfBirth');
    const genderInput = document.getElementById('Gender');
    const phoneInput = document.getElementById('Cellphone');
    const emailInput = document.getElementById('Email');

    const progressSteps = Array.from(document.querySelectorAll('.inline-progress__step[data-step-target]'));
    const sections = Array.from(document.querySelectorAll('.form-section[data-step-section]'));

    function titleCase(value) {
        return value
            .toLowerCase()
            .replace(/\b\w/g, function (m) { return m.toUpperCase(); });
    }

    function detectIdentityType(value) {
        const trimmed = value.trim();
        if (!trimmed) return 'empty';
        if (/^\d{13}$/.test(trimmed)) return 'id';
        if (/^[A-Za-z0-9\-\/]{6,20}$/.test(trimmed)) return 'passport';
        return 'unknown';
    }

    function setLookupState() {
        if (!lookupInput || !lookupState || !lookupHelp || !lookupFormatBadge) {
            return;
        }

        const value = lookupInput.value.trim();
        const detected = detectIdentityType(value);

        if (!value) {
            lookupState.textContent = 'Waiting for input';
            lookupState.className = 'field-state info';
            lookupHelp.textContent = 'Enter a South African ID number or passport reference.';
            lookupFormatBadge.innerHTML = '<i class="fa fa-wand-magic-sparkles"></i> Format not detected yet';
            return;
        }

        if (detected === 'id') {
            lookupState.textContent = 'SA ID format detected';
            lookupState.className = 'field-state valid';
            lookupHelp.textContent = '13-digit identity number detected. Ready for applicant lookup.';
            lookupFormatBadge.innerHTML = '<i class="fa fa-id-card"></i> South African ID format';
            return;
        }

        if (detected === 'passport') {
            lookupState.textContent = 'Passport format detected';
            lookupState.className = 'field-state valid';
            lookupHelp.textContent = 'Passport-style reference detected. Lookup can continue.';
            lookupFormatBadge.innerHTML = '<i class="fa fa-earth-africa"></i> Passport format';
            return;
        }

        lookupState.textContent = 'Check format';
        lookupState.className = 'field-state warn';
        lookupHelp.textContent = 'The value looks unusual. You can still continue if it is correct.';
        lookupFormatBadge.innerHTML = '<i class="fa fa-triangle-exclamation"></i> Unrecognised format';
    }

    function parseSouthAfricanId(idNumber) {
        if (!/^\d{13}$/.test(idNumber)) {
            return null;
        }

        const yy = parseInt(idNumber.substring(0, 2), 10);
        const mm = parseInt(idNumber.substring(2, 4), 10);
        const dd = parseInt(idNumber.substring(4, 6), 10);
        const genderDigits = parseInt(idNumber.substring(6, 10), 10);

        const currentTwoDigitYear = new Date().getFullYear() % 100;
        const fullYear = yy <= currentTwoDigitYear ? 2000 + yy : 1900 + yy;

        const date = new Date(fullYear, mm - 1, dd);
        const isValidDate =
            date.getFullYear() === fullYear &&
            date.getMonth() === mm - 1 &&
            date.getDate() === dd;

        if (!isValidDate) {
            return null;
        }

        return {
            dateOfBirth: `${fullYear}-${String(mm).padStart(2, '0')}-${String(dd).padStart(2, '0')}`,
            gender: genderDigits >= 5000 ? 'Male' : 'Female'
        };
    }

    function syncIdPassportExclusivity() {
        if (!idInput || !passportInput) {
            return;
        }

        const hasId = idInput.value.trim().length > 0;
        const hasPassport = passportInput.value.trim().length > 0;

        if (hasId) {
            passportInput.value = '';
            passportInput.setAttribute('disabled', 'disabled');
            updateFieldState(passportInput, '', null);
        } else {
            passportInput.removeAttribute('disabled');
        }

        if (hasPassport) {
            idInput.value = '';
            idInput.setAttribute('disabled', 'disabled');
            updateFieldState(idInput, '', null);
        } else {
            idInput.removeAttribute('disabled');
        }
    }

    function syncIdDerivedFields() {
        if (!idInput) {
            return;
        }

        const parsed = parseSouthAfricanId(idInput.value.trim());

        if (!parsed) {
            if (dobInput && !dobInput.dataset.lockedByLookup) {
                dobInput.value = '';
            }

            if (genderInput && !genderInput.dataset.lockedByLookup) {
                genderInput.value = '';
                genderInput.dispatchEvent(new Event('change'));
            }

            return;
        }

        if (dobInput && !dobInput.dataset.lockedByLookup) {
            dobInput.value = parsed.dateOfBirth;
            updateFieldState(dobInput, '', true);
        }

        if (genderInput && !genderInput.dataset.lockedByLookup) {
            genderInput.value = parsed.gender;
            genderInput.classList.add('has-value');
            genderInput.dispatchEvent(new Event('change'));
            updateFieldState(genderInput, '', true);
        }

        updateFieldState(idInput, '', true);
    }

    function goToStep(stepName) {
        if (!stepName) {
            return;
        }

        progressSteps.forEach(function (step) {
            const isActive = step.dataset.stepTarget === stepName;
            step.classList.toggle('active', isActive);
        });

        sections.forEach(function (section) {
            const isActive = section.dataset.stepSection === stepName;
            section.classList.toggle('is-hidden', !isActive);
            section.classList.toggle('is-active', isActive);
        });
    }

    function updateFieldState(field, message, isValid) {
        if (!field || !field.name) {
            return;
        }

        const target = document.querySelector('[data-state-for="' + field.name + '"]');
        if (!target) {
            return;
        }

        if (!message && isValid === true) {
            target.innerHTML = '<span class="field-state-icon valid" title="Valid"><i class="fa fa-circle-check"></i></span>';
            target.className = 'field-state valid icon-only';
            return;
        }

        if (!message) {
            target.textContent = '';
            target.className = 'field-state';
            return;
        }

        target.textContent = message;

        if (isValid === true) {
            target.className = 'field-state valid';
            return;
        }

        if (isValid === false) {
            target.className = 'field-state warn';
            return;
        }

        target.className = 'field-state info';
    }

    if (lookupInput) {
        lookupInput.addEventListener('input', setLookupState);
        lookupInput.addEventListener('blur', setLookupState);
        setLookupState();
    }

    document.querySelectorAll('.person-name').forEach(function (input) {
        input.addEventListener('blur', function () {
            if (this.value && this.value.trim()) {
                this.value = titleCase(this.value.trim());
            }
        });
    });

    if (idInput) {
        idInput.addEventListener('input', function () {
            this.value = this.value.replace(/\D/g, '').substring(0, 13);
            syncIdPassportExclusivity();
            syncIdDerivedFields();
        });

        idInput.addEventListener('blur', function () {
            syncIdDerivedFields();
            if (this.value.trim() && /^\d{13}$/.test(this.value.trim())) {
                updateFieldState(this, '', true);
            }
        });
    }

    if (passportInput) {
        passportInput.addEventListener('input', function () {
            syncIdPassportExclusivity();
        });

        passportInput.addEventListener('blur', function () {
            if (this.value.trim()) {
                updateFieldState(this, '', true);
            }
        });
    }

    if (phoneInput) {
        phoneInput.addEventListener('input', function () {
            let value = this.value.replace(/[^\d+]/g, '');

            if (value.startsWith('+27')) {
                value = '+27 ' + value.substring(3).replace(/\D/g, '').substring(0, 9);
            } else {
                value = value.replace(/\D/g, '').substring(0, 10);
            }

            this.value = value;
        });

        phoneInput.addEventListener('blur', function () {
            if (this.value.trim()) {
                updateFieldState(this, '', true);
            }
        });
    }

    if (emailInput) {
        emailInput.addEventListener('input', function () {
            const value = this.value.trim();
            if (!value) {
                updateFieldState(this, '', null);
                return;
            }

            const ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
            updateFieldState(this, ok ? '' : 'Check email format', ok);
        });
    }

    document.querySelectorAll('.form-select').forEach(function (select) {
        const sync = function () {
            if (select.value) {
                select.classList.add('has-value');
            } else {
                select.classList.remove('has-value');
            }
        };

        select.addEventListener('change', function () {
            sync();
            updateFieldState(this, this.value ? '' : '', this.value ? true : null);
        });

        sync();
    });

    document.querySelectorAll('#enrollmentForm .form-control, #enrollmentForm .form-select').forEach(function (field) {
        field.addEventListener('blur', function () {
            const value = (field.value || '').trim();
            const required = field.hasAttribute('data-val-required') || field.required;

            if (!value) {
                updateFieldState(field, required ? 'Required' : '', null);
                return;
            }

            if (field.checkValidity()) {
                updateFieldState(field, '', true);
            } else {
                updateFieldState(field, 'Check this field', false);
            }
        });
    });

    progressSteps.forEach(function (step) {
        step.addEventListener('click', function () {
            goToStep(step.dataset.stepTarget);
        });
    });

    if (sections.length > 0) {
        goToStep('identity');
    }

    syncIdPassportExclusivity();

    if (sections.length > 0) {
        goToStep('identity');
    }

    syncIdPassportExclusivity();
    syncIdDerivedFields();

    if (idInput && idInput.value.trim()) {
        const parsed = parseSouthAfricanId(idInput.value.trim());
        if (parsed) {
            updateFieldState(idInput, '', true);
        }
    }

    if (passportInput && passportInput.value.trim()) {
        updateFieldState(passportInput, '', true);
    }

    if (phoneInput && phoneInput.value.trim()) {
        updateFieldState(phoneInput, '', true);
    }

    if (emailInput && emailInput.value.trim()) {
        const ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailInput.value.trim());
        updateFieldState(emailInput, ok ? '' : 'Check email format', ok);
    }

    document.querySelectorAll('.form-select').forEach(function (select) {
        if (select.value) {
            select.classList.add('has-value');
            updateFieldState(select, '', true);
        }
    });
});