// Mobile sidebar toggle
(function () {
    const sidebar = document.getElementById('sidebar');
    const toggle = document.getElementById('sidebarToggle');
    const backdrop = document.getElementById('sidebarBackdrop');
    if (!sidebar || !toggle || !backdrop) return;

    const close = () => document.body.classList.remove('sidebar-open');
    toggle.addEventListener('click', () => document.body.classList.toggle('sidebar-open'));
    backdrop.addEventListener('click', close);
})();

// Create form: show a receiver hint matching the selected notification type
(function () {
    const select = document.getElementById('typeSelect');
    const input = document.getElementById('receiverInput');
    const hint = document.getElementById('receiverHint');
    if (!select || !input || !hint) return;

    const hints = {
        Email: { placeholder: 'name@example.com', text: 'Enter a valid email address.' },
        Sms: { placeholder: '+905551234567', text: 'Enter a phone number (7-15 digits, optional leading +).' },
        WhatsApp: { placeholder: '+905551234567', text: 'Enter a WhatsApp phone number (7-15 digits, optional leading +).' },
        Push: { placeholder: 'device-token-1234', text: 'Enter a device token (at least 8 characters, no spaces).' }
    };

    const update = () => {
        const h = hints[select.value];
        input.placeholder = h ? h.placeholder : 'Receiver';
        hint.textContent = h ? h.text : 'Choose a type to see the expected receiver format.';
    };

    select.addEventListener('change', update);
    update();
})();
