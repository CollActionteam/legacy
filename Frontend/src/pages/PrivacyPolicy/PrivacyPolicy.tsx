import React from 'react'
import { Section } from '../../components/Section/Section'
import styles from "./PrivacyPolicy.module.scss";
import { Helmet } from 'react-helmet';

const PrivacyPolicyPage = () => { 
    return <>
        <Helmet>
            <title>Privacy Policy</title>
            <meta name="description" content="Privacy Policy" />
        </Helmet>
        <Section className={styles.intro}>
            <h1 className={styles.title}>Privacy and Cookie Policy CollAction</h1>
        </Section>
        <Section color="grey">
            <p>
                We encourage you to read this privacy and cookie policy (“<b>Policy</b>”) carefully before using our website, as it provides important information about how and why we gather and process personal information (“<b>Personal Information</b>”) about you when you use our website.
                Personal Information is information that can directly or indirectly be used to identify you.
            </p>
            <p>
                Stichting CollAction (“<b>CollAction</b>”) is the controller of the data collected and processed, as is defined in EU Regulation 2016/679 (“<b>GDPR</b>”).
                Our contact details can be found further below.
            </p>
            <p>
                This Policy applies to our website only.
                It does not apply to other companies or organizations that for example advertise our website, crowdactions or services, or to other (social) websites, networks and platforms that you can connect to through our website.
                We are not responsible or liable for any processing of your personal information via these (social) websites, networks and platforms. We also advise you to read their privacy and cookie policies carefully before using their websites.
            </p>
            <h3>
                Why does CollAction collect and process my Personal Information? And which Personal Information does CollAction collect and process?
            </h3>
            <p>
                We collect and process your Personal Information in order to:
            </p>
            <ol>
                <li>create a CollAction account for you at your request; </li>
                <li>sign you up for crowdactions or enable you to start your own crowdaction at your request;</li>
                <li>keep in contact with you and update you on the progress of certain crowdactions;</li>
                <li>advertise and promote crowdactions, our website, activities and other services to you, for example by sending you newsletters if you opted in to receive our newsletters;</li>
                <li>comply with legal obligations; and </li>
                <li>further develop and improve our website and services.</li>
            </ol>
            <p>
                In order to do the above, we collect and process Personal Information, such as your name and e-mail address.
                We may also collect your IP address and information on your use of our website by using cookies (text files placed on your computer) and other technologies such as Google Analytics and Google Tag Manager in order to further develop and improve our website and the overall quality of our services.
                For more information on the use of cookies and other technologies, please see ‘Use of cookies’ further below.
            </p>
            <p>
                Our processing of your Personal Information is based on one or more of the following legal grounds:
            </p>
            <ol>
                <li>
                    you’ve consented to the processing of your Personal Information (Article 6 (1) sub a GDPR);
                </li>
                <li>
                    the processing of your Personal Information is necessary for the performance of a contract between you and us or for taking any pre-contractual steps upon your request (Article 6 (1) sub b GDPR).
                    If this Personal Information is not processed, we will not be able to execute the contract with you, i.e. provide you certain services you requested (e.g. creating an account for you);
                </li>
                <li>
                    the processing is necessary for us to comply with a legal obligation (Article 6 (1) sub c GDPR); and/or
                </li>
                <li>
                    the processing is necessary for the purposes of our legitimate interests, for example to detect fraud (Article 6 (1) sub f GDPR).
                </li>
            </ol>
            <h3>How long does CollAction store my Personal Information?</h3>
            <p>
                CollAction does not store or keep your Personal Information stored longer than necessary for the purposes for which the Personal Information was processed.
                Nevertheless, you have the right to change how we use/collect your Personal Information at any time (see how to opt-out further below), unless we are required to retain this Personal Information by law or to comply with our regulatory obligations.
            </p>
            <h3>Does CollAction share my Personal Information with third parties?</h3>
            <p>
                CollAction is a non-profit organization dedicated to making the world a better place with the help of individuals like you. In accordance with our mission, CollAction will not sell your Personal Information to third parties.
            </p>
            <p>
                As a non-profit organization that uses open source, we do however have a team of volunteers located all over the world that help us in providing the best service possible.
                In doing so, your Personal Information may be exchanged, transferred or otherwise made available to these volunteers.
                Appropriate agreements will be made with these volunteers to ensure the safeguarding of your Personal Information.
            </p>
            <p>
                Given that our volunteers are located all over the world, your Personal Information may also be transmitted to and/or made accessible to countries outside the European Economic Area, in some cases to countries that do not provide an adequate level of data protection according to the European Commission.
                In that case, Personal Information will only be transferred under the following conditions:
            </p>
            <ol>
                <li>you have explicitly consented to the proposed transfer; </li>
                <li>the transfer is necessary for the performance of the contract, i.e. the provision of our services; or </li>
                <li>the transfer is necessary for the performance of a contract concluded in your interest.</li>
            </ol>
            <p>
                We may also release your Personal Information when we believe such release is necessary in order to comply with applicable law, enforce the website’s policies, protect CollAction’s legal rights, property, or safety, or those of third parties.
                Information that is not personally identifiable may also be shared publicly, for example for statistical purposes.
            </p>
            <p>
                CollAction uses the Cloud server services of Amazon (Amazon Web Services).
                Therefore, your Personal Information may be transferred to and stored in servers located in the United States.
                Amazon Web Services is certified under the EU–US Privacy Shield, which is a framework that provides companies in the EU and the United States with a mechanism to comply with data protection requirements when transferring personal data from the EU to the United States.
            </p>
            <h3>Use of cookies</h3>
            <p>
                Cookies are small text files that are placed on your device when you visit a website, and can have a wide range of functions. CollAction uses the following types of cookies:
            </p>
            <h4>Strictly necessary/ functional cookies</h4>
            <p>
                These are cookies that are necessary for the (basic) operation of our website, for example to keep you logged in.
            </p>
            <h4>Analytics cookies</h4>
            <p>
                CollAction makes use of third party analytics services that use cookies, such as Google Analytics, Google Tag Manager and Facebook Pixel.
                These are used to obtain relevant statistics and other information about the use of our website in order to enhance the performance of our website and the provision of our services.
                These third party services may place cookies, including tracking and advertising cookies.
            </p>
            <p>
                Note that the settings for Google Analytics are set to a privacy friendly mode, so that your IP address is anonymized before information is shared with Google.
            </p>
            <h4>Social media buttons and embedded YouTube videos</h4>
            <p>
                We have placed ‘social media buttons’ on our website which allow you to connect to various social media platforms such as Facebook and Twitter to, for example, share a link.
                This is made possible by codes that are delivered by the relevant social media platforms.
                These codes place cookies. We are not responsible for these cookies. We encourage you to read the privacy policies of the relevant social media platforms for more information on how they treat your Personal Information.
            </p>
            <p>
                Our website also displays YouTube videos, which you can watch on our website. By playing these videos, you accept the cookies used and placed by YouTube. We do not control these cookies and refer you to the cookie and privacy policy of YouTube for more privacy related information.
            </p>
            <h4>Advertising cookies</h4>
            <p>
                As mentioned above, we make use of third party analytics services and have added social media buttons on our website. As a result, third party cookies may be placed on your device, including cookies for advertisement purposes. We note that we are not responsible for the placement of these cookies.
            </p>
            <h4>Disabling cookies</h4>
            <p>
                If you wish to disable the use and placement of cookies, we advise you to amend your internet browser settings. We do however note that disabling or deleting certain cookies may mean that you can no longer access or use certain features of our website or will reduce the overall functioning of our website.
            </p>
            <p>
                For more information on how to disable the use of Google Analytics specifically, please see the following link: https://tools.google.com/dlpage/gaoptout.
            </p>
            <h3>How to opt-out</h3>
            <p>
                You may revoke any consent given to us with regard to the collection, processing and use of your Personal Information at any time for the future and also generally request that CollAction stop using and collecting your Personal Information (“opt out”).
            </p>
            <p>
                To opt out, please submit a request to CollAction via e-mail or by post with the subject “Opt-Out request CollAction”. CollAction’s contact details can be found below.
                Upon receipt, verification and confirmation of your request, CollAction will process your request as soon as possible. Note that this does not automatically include opting-out from our newsletters.
            </p>
            <p>
                To opt-out from our newsletters only, click on ‘unsubscribe from the mailing list’ at the bottom of the newsletter.
                This opt-out does not apply to communications that are pertinent to the services you have requested or participate in or where we are required to provide you with notifications (such as a notice of an update to our Policy).
            </p>
            <h3>What other rights do I have?</h3>
            <p>
                Subject to certain legal limitations, you can at all times submit a request to CollAction to do the following:
            </p>
            <ul>
                <li>gain access to your Personal Information; </li>
                <li>rectify or erase your Personal Information; </li>
                <li>restrict processing of your Personal Information or object to such processing; and </li>
                <li>transfer your Personal Information to another controller by sending this in a structured and standard format.</li>
            </ul>
            <p>
                Participants also have the right to lodge a complaint against CollAction with the Dutch supervisory authority (Autoriteit Persoonsgegevens) regarding the processing of your Personal Information.
            </p>
            <h3>Security</h3>
            <p>
                CollAction is committed to protecting your Personal Information and will take all reasonable precautions to do so.
                Though we can never guarantee full protection, we do regularly review our information collection, storage and processing practices to see if additional safeguarding measures should be taken.
            </p>
            <h3>Contact details</h3>
            <p>
                If you have any concerns or requests, please direct these to:
            </p>
            <address>
                Stichting CollAction<br />
                Admiraal De Ruijterweg 309-4 <br />
                1055 LX Amsterdam
            </address>
            <p>
                or
            </p>
            <p>
                <a href="mailto:hello@collaction.org">hello@collaction.org</a>
            </p>
            <p>
                We will respond to your request in a reasonable timeframe, and in any event within 30 days after receipt.
            </p>
            <h3>Changes to our Policy</h3>
            <p>
                We may need to amend or update our Policy from time to time.
                This will replace any previous Policy.
                Should we make any material changes to our Policy, we will notify you accordingly through the website and other places that we deem appropriate.
                We also encourage you to read our Policy regularly in order to stay updated.
            </p>
            <p>
                Updated: 14-06-2019
            </p>
        </Section>
    </>;
}

export default PrivacyPolicyPage;