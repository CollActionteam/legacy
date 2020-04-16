import React from 'react';
import { Grid } from '@material-ui/core';

import styles from './About.module.scss';
import { Section } from '../../components/Section/Section';
import { Faq } from '../../components/Faq/Faq';

import { useTranslation } from 'react-i18next';
import Helmet from 'react-helmet';

const AboutPage = () => {
  const { t } = useTranslation();
  const faqs: any[] = t('about.faqs', { returnObjects: true });
  const team: any[] = t('about.team.team', { returnObjects: true });
  
  const videos = {
    mainvideo: 'https://www.youtube-nocookie.com/embed/xnIJo91Gero?theme=dark&amp;rel=0&amp;wmode=transparent'
  };

  const generateMemberPhoto = (member: any) => {
    const photo = require(`../../assets/${member.photo}`);
    return (
      <li key={member.name} className={styles.teamMember}>
        <img src={photo} alt={member.name} title={member.name} />
        <span>{member.name}</span>
      </li>
    );
  };

  return (
    <React.Fragment>
      <Helmet>
        <title>About CollAction</title>
        <meta name="description" content="About Collaction" />
      </Helmet>
      <Grid className={styles.video}>
        <iframe
          title="Collective actions"
          src={videos.mainvideo}
          frameBorder="0"
          allowFullScreen
        ></iframe>
      </Grid>
      <Section color="green" title={(t('about.mission.title'))} anchor="mission">
        <span dangerouslySetInnerHTML={{ __html: t('about.mission.text') }}></span>
      </Section>
      <Section title={t('about.about.title')}>
        <span dangerouslySetInnerHTML={{ __html: t('about.about.text') }}></span>
      </Section>
      <Section color="grey" title={t('about.team.title')} anchor="team">
        <ul className={styles.team}>
          {team.map(generateMemberPhoto)}
        </ul>
      </Section>
      <Section title={t('about.join.title')}>
        <span dangerouslySetInnerHTML={{ __html: t('about.join.text') }}></span>
      </Section>
      <Section color="grey" title={t('about.partners.title')} anchor="partners">
        <span dangerouslySetInnerHTML={{ __html: t('about.partners.text') }}></span>
      </Section>
      <Section title="Frequently Asked Questions" anchor="faq">
        {faqs.map((faq) => (
          <Faq key={faq.title} title={faq.title}>{faq.text}</Faq>
        ))}
      </Section>
    </React.Fragment>
  );
};

export default AboutPage;
