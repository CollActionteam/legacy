import renderComponentIf from "./renderComponentIf";
import * as React from "react";
import * as ReactDOM from 'react-dom';
import * as Modal from 'react-modal';
import {CardElement, IdealBankElement, StripeProvider, Elements, injectStripe} from 'react-stripe-elements';

interface IDonationBoxProps {
    stripe: any;
}

interface IDonationBoxState {
    amount: number;
    showError: boolean;
    paymentMethod: PaymentMethod;
    showPopup: boolean;
    name: string;
}

enum PaymentMethod {
    None,
    iDeal,
    CreditCard
}

const popupStyles = {
  content : {
    top                   : '50%',
    left                  : '50%',
    right                 : 'auto',
    bottom                : 'auto',
    marginRight           : '-50%',
    transform             : 'translate(-50%, -50%)'
  }
};

class DonationBox extends React.Component<IDonationBoxProps, IDonationBoxState> {
    constructor(props) {
        super(props);
        this.state = { 
            amount: 0, 
            showError: false, 
            showPopup: false, 
            paymentMethod: PaymentMethod.None, 
            name: ""
        };
        Modal.setAppElement('#site-content');
    }

    setAmount = (event) => {
        let newAmount = Number.parseInt(event.currentTarget.value);
        if (!newAmount) {
            newAmount = 0;
        }
        this.setState({amount: newAmount, showError: false});
    }

    setName = (event) => {
        this.setState({name: event.currentTarget.value});
    }

    pay = (method: PaymentMethod) => {
        if (this.state.amount <= 0) {
            this.setState({ showError: true});
            return;
        }
        this.setState({paymentMethod: method, showPopup: true});
    }

    payIdeal = () => {
        this.pay(PaymentMethod.iDeal);
    }

    payCreditCard = () => {
        this.pay(PaymentMethod.CreditCard);
    }

    renderAmount(amount: number) {
        const id = `amount${amount}`;
        return (
            <div className="col-xs-6 col-sm-3">
                <input id={id} type="radio" name="amount" value={amount} onChange={(event) => this.setAmount(event)} checked={this.state.amount === amount} />
                <label htmlFor={id}>&euro; {amount}</label>
            </div>
        );
    }

    renderCustomAmount() {
        return (
            <div className="col-xs-6 col-sm-3">
                <input type="text" id="amountCustom" name="amount" placeholder="Other..." onChange={(event) => this.setAmount(event)} />
            </div>
        );
    }

    closePayment() {
        this.setState({showPopup: false, paymentMethod: PaymentMethod.None});
    }

    showPopup() {
        return this.state.paymentMethod != PaymentMethod.None;
    }

    async submitPayment() {
        let { stripeToken } = await this.props.stripe.createToken({name: this.state.name});
        let response = await fetch(`/Donation/PerformDonation?name=${encodeURIComponent(this.state.name)}&token=${stripeToken}`, {
            method: "POST"
        });
    }

    renderPaymentPopup() {
        return (
            <Modal isOpen={this.state.showPopup} onRequestClose={() => this.closePayment()} contentLabel="Donate" style={popupStyles}>
                <div id="donation-modal">
                    <label htmlFor="name-input">Name</label>
                    <input id="name-input" className="form-control" onChange={(event) => this.setName} type="text" />
                    {this.renderStripe()}
                    <a className="btn btn-default" onClick={() => this.submitPayment()}>Submit</a>
                    <a className="btn" onClick={() => this.closePayment()}>Close</a>
                </div>
            </Modal>
        );
    }

    renderStripe() {
        if (this.state.paymentMethod == PaymentMethod.CreditCard) {
            return (
                <CardElement />
            );
        }
        else {
            return (
                <IdealBankElement />
            );
        }
    }

    render() {
        return (
            <div className="donation-box">
                {this.renderPaymentPopup()}
                <h2>Help us reaching our mission by donating!</h2>
                <p>
                    By donating to CollAction you help us solve collactive action problems and to make the world a better place. Your donation helps us maintaining and growing the platform and ultimately multiply our impact.
                </p>
                <hr />
                <p>
                    If you'd like make a one-time donation, please select the amount and payment option.
                </p>
                <div className="row">
                    {this.renderAmount(3)}
                    {this.renderAmount(5)}
                    {this.renderAmount(10)}
                    {this.renderAmount(20)}
                    {this.renderAmount(30)}
                    {this.renderAmount(50)}
                    {this.renderAmount(100)}
                    {this.renderCustomAmount()}
                </div>
                <div className="error" hidden={!this.state.showError}>
                    <p>Thank you! Please select or enter an amount you'd like to donate.</p>
                </div>
                <div className="payment-options">
                    <div className="row">
                        <div className="col-xs-12">
                            <button className="btn" value="ideal" onClick={() => this.payIdeal()}>
                                <img src="/images/thankyoucommit/iDEAL-logo.png" />
                                <div>iDEAL</div>
                            </button>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-xs-12">
                            <button className="btn" value="creditcard" onClick={() => this.payCreditCard()}>
                                <img src="/images/thankyoucommit/bank-card.png" />
                                <div>Debit / Creditcard</div>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

const InjectedDonationBox = injectStripe(DonationBox);

let donationBox: HTMLElement = document.getElementById("donation-box");
if (donationBox !== null) {
    let stripePublicKey: string = donationBox.getAttribute("data-stripe-public-key");
    renderComponentIf(
        <StripeProvider apiKey={stripePublicKey}>
            <Elements>
                <InjectedDonationBox />
            </Elements>
        </StripeProvider>,
        donationBox
    );
}