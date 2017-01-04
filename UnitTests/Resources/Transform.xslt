<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:xdt="http://www.w3.org/2005/xpath-datatypes">
	<xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes"/>
	<xsl:template match="/list">
		<ul>
			<xsl:call-template name="items" />
		</ul>
	</xsl:template>
	<xsl:template name="items">
		<xsl:for-each select="item">
			<li><xsl:value-of select="@name" />=<xsl:value-of select="@value" /></li>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
