import React from 'react';
import { Grid } from '@material-ui/core';

import styles from './About.module.scss';
import { Section } from '../../components/Section';
import { Faq } from '../../components/Faq';

import { useTranslation } from 'react-i18next';

const AboutPage = () => {
  const { t } = useTranslation();
  const faqs: any[] = t('about.faqs', { returnObjects: true });
  const team: any[] = t('about.team.team', { returnObjects: true });
  
  const videos = {
    mainvideo: ''
  };

  const generateMemberPhoto = (member: any) => (
    <li key={member.name} className={styles.teamMember}>
      <img src={member.photo} alt={member.name} title={member.name} />
      <span>{member.name}</span>
    </li>
  );

  return (
    <React.Fragment>
      <Grid className={styles.video}>
        <iframe
          title="Collective actions"
          src={videos.mainvideo}
          frameBorder="0"
          allowFullScreen
        ></iframe>
      </Grid>
      <Section color="green" title={(t('about.mission.title'))}>
        <span dangerouslySetInnerHTML={{ __html: t('about.mission.text') }}></span>
      </Section>
      <Section title={t('about.about.title')}>
        <span dangerouslySetInnerHTML={{ __html: t('about.about.text') }}></span>
      </Section>
      <Section color="grey" title={t('about.team.title')}>
        <ul className={styles.team}>
          {team.map(generateMemberPhoto)}
        </ul>
      </Section>
      <Section title={t('about.join.title')}>
        <span dangerouslySetInnerHTML={{ __html: t('about.join.text') }}></span>
      </Section>
      <Section color="grey" title={t('about.partners.title')}>
        <span dangerouslySetInnerHTML={{ __html: t('about.partners.text') }}></span>
      </Section>
      <Section title="Frequently Asked Questions">
        {faqs.map((faq) => (
          <Faq key={faq.title} title={faq.title} content={faq.text}></Faq>
        ))}
      </Section>
    </React.Fragment>
  );
};

export default AboutPage;
